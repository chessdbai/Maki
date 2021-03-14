import * as cdk from '@aws-cdk/core';
import * as ec2 from '@aws-cdk/aws-ec2';
import * as iam from '@aws-cdk/aws-iam';
import * as s3 from '@aws-cdk/aws-s3';
import * as s3deploy from '@aws-cdk/aws-s3-deployment';
import * as ami from '@aws-cdk/aws-imagebuilder';
import * as fs from 'fs';
import * as path from 'path';

const getComponentData = (name: string) => {
  const fullPath = path.join(__dirname, `components/${name}.yaml`);
  const data = fs.readFileSync(fullPath);
  return data.toString('utf-8');
}

interface AmiPipelineProps {
  vpc: ec2.IVpc
}

export class AmiPipeline extends cdk.Construct {

  constructor(parent: cdk.Construct, name: string, props: AmiPipelineProps) {
    super(parent, name);


    const amiResourcesBucket = new s3.Bucket(this, 'AmiResources', {
      bucketName: 'chessdb-maki-ami-resources',
      removalPolicy: cdk.RemovalPolicy.DESTROY
    });
    const deployment = new s3deploy.BucketDeployment(this, 'Deployment', {
      destinationBucket: amiResourcesBucket,
      sources: [
        s3deploy.Source.asset(path.join(__dirname, 'components/config'))
      ]
    });
    const amiBuilderRole = new iam.Role(this, 'Role', {
      assumedBy: new iam.ServicePrincipal('ec2.amazonaws.com')
    });
    amiBuilderRole.addManagedPolicy(iam.ManagedPolicy.fromAwsManagedPolicyName('AmazonSSMFullAccess'));
    amiBuilderRole.addManagedPolicy(iam.ManagedPolicy.fromAwsManagedPolicyName('CloudWatchLogsFullAccess'));
    amiBuilderRole.addToPolicy(new iam.PolicyStatement({
      actions: [ '*' ],
      resources: [ '*' ]
    }))
    amiResourcesBucket.grantRead(amiBuilderRole);

    const instanceProfile = new iam.CfnInstanceProfile(this, 'InstanceProfile', {
      roles: [
        amiBuilderRole.roleName
      ],
      instanceProfileName: 'AmiBuilderInstanceProfile'
    });

    const distributionConfig = new ami.CfnDistributionConfiguration(this, 'DistroConfig', {
      name: 'EngineDaemonDistribution',
      distributions: [
        {
          region: 'us-east-2',
          amiDistributionConfiguration: {
            "Name": "Maki - {{ imagebuilder:buildDate }}",
            "Description": "AMI capable of running docker containers with the Maki engine daemon.",
            "LaunchPermissionConfiguration": {
              "UserIds": [
                "541249553451"
              ]
            }
          },
        }
      ]
    });

    const cudaComponent = new ami.CfnComponent(this, 'CudaComponent', {
      name: 'Cuda112',
      description:  "Installs Nvidia CUDA 11.2",
      platform: 'Linux',
      data: getComponentData('cuda'),
      version: '11.2.0'
    });

    const cudnnComponent = new ami.CfnComponent(this, 'CudNNComponent', {
      name: 'CudNN811',
      description:  "Installs Nvidia CudNN 8.1.1",
      platform: 'Linux',
      data: getComponentData('cudnn'),
      version: '8.1.1'
    });

    const dockerComponent = new ami.CfnComponent(this, 'DockerComponent', {
      name: 'DockerWithNvidiaRuntime',
      description:  "Installs Docker with the Nvidia runtime.",
      platform: 'Linux',
      data: getComponentData('docker_nvidia'),
      version: '1.0.0'
    });

    const collectorComponent = new ami.CfnComponent(this, 'CollectorComponent', {
      name: 'ContainerCollectorAgentConfig',
      description:  "Sets up the CloudWatch Logs agent config file to listen to container telemetry.",
      platform: 'Linux',
      data: getComponentData('collector'),
      version: '1.0.0'
    });


    const imageRecipe = new ami.CfnImageRecipe(this, 'Recipe', {
      name: 'maki',
      version: '1.0.0',
      parentImage: 'ami-08962a4068733a2b6',
      description: 'Recipe to build the Maki AMI.',
      components: [
        {
          componentArn: `arn:aws:imagebuilder:${cdk.Aws.REGION}:aws:component/amazon-cloudwatch-agent-linux/1.0.0`,
        },
        {
          componentArn: `arn:aws:imagebuilder:${cdk.Aws.REGION}:aws:component/aws-cli-version-2-linux/1.0.0`,
        },
        {
          componentArn: `arn:aws:imagebuilder:${cdk.Aws.REGION}:aws:component/dotnet-runtime-linux/5.0.0`,
        },
        {
          componentArn: cudaComponent.ref,
        },
        {
          componentArn: cudnnComponent.ref,
        },
        {
          componentArn: dockerComponent.ref,
        },
        {
          componentArn: collectorComponent.ref
        }
      ],
      blockDeviceMappings: [
        {
          deviceName: '/dev/sda1',
          ebs: {
            deleteOnTermination: true,
            encrypted: false,
            volumeType: 'gp2',
            volumeSize: 256
          }
        }
      ]
    });

    const buildLogsBucket = new s3.Bucket(this, 'BuildLogsBucket', {
      bucketName: 'chessdb-maki-ami-build-logs',
      removalPolicy: cdk.RemovalPolicy.DESTROY
    });
    buildLogsBucket.grantReadWrite(amiBuilderRole);

    const amiInstanceSecurityGroup = new ec2.SecurityGroup(this, 'AmiInstanceSecurityGroup', {
      description: 'Security group used by the instance while building the AMI.',
      vpc: props.vpc,
      allowAllOutbound: true
    });
    amiInstanceSecurityGroup.addIngressRule(ec2.Peer.ipv4(props.vpc.vpcCidrBlock), ec2.Port.allTraffic());

    const infraConfig = new ami.CfnInfrastructureConfiguration(this, 'InfraConfig', {
      name: 'EngineDaemonConfig',
      instanceProfileName: instanceProfile.instanceProfileName!,
      instanceTypes: [
        'p3.2xlarge'
      ],
      logging: {
        s3Logs: {
          s3BucketName: buildLogsBucket.bucketName
        }
      },
      subnetId: props.vpc.privateSubnets[0].subnetId,
      securityGroupIds: [
        amiInstanceSecurityGroup.securityGroupId
      ]
    });
    infraConfig.addDependsOn(instanceProfile);

    const buildPipeline = new ami.CfnImagePipeline(this, 'Pipeline', {
      name: 'engine-daemon-pipeline',
      enhancedImageMetadataEnabled: true,
      description: 'Pipeline to build the engine daemon AMI.',
      distributionConfigurationArn: distributionConfig.ref,
      imageRecipeArn: imageRecipe.ref,
      infrastructureConfigurationArn: infraConfig.ref,
      imageTestsConfiguration: {
        imageTestsEnabled: false,
        timeoutMinutes: 90
      },
      status: 'ENABLED'
    });
    buildPipeline.addDependsOn(infraConfig);
    buildPipeline.addDependsOn(imageRecipe);
    buildPipeline.addDependsOn(distributionConfig);
  }
}