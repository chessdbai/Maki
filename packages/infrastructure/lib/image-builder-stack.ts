import * as cdk from '@aws-cdk/core';
import * as iam from '@aws-cdk/aws-iam';
import * as ec2 from '@aws-cdk/aws-ec2';

import { AmiPipeline } from './ami-pipeline';

export class ImageBuilderStack extends cdk.Stack {
  constructor(scope: cdk.Construct, id: string, props?: cdk.StackProps) {
    super(scope, id, props);

    const network = new ec2.Vpc(this, 'AmiBuildVpc', {
      subnetConfiguration: [
        {
          subnetType: ec2.SubnetType.PUBLIC,
          name: 'public'
        },
        {
          subnetType: ec2.SubnetType.PRIVATE,
          name: 'private'
        },
      ],
      maxAzs: 1
    });

    new AmiPipeline(this, 'Pipeline', {
      vpc: network
    });
  }
}
