//-----------------------------------------------------------------------
// <copyright file="CloudClientFactory.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Cloud
{
    using System;
    using Amazon;
    using Amazon.Runtime;

    /// <summary>
    /// Class to create instances of AWS service clients based on a loaded CloudConfig.
    /// </summary>
    public static class CloudClientFactory
    {
        private static readonly Lazy<Type> CredentialsTypeLazy = new Lazy<Type>(() => typeof(AWSCredentials));
        private static readonly Lazy<Type> RegionTypeLazy = new Lazy<Type>(() => typeof(RegionEndpoint));

        private static Type CredentialsType => CredentialsTypeLazy.Value;

        private static Type RegionType => RegionTypeLazy.Value;

        /// <summary>
        /// Create a new AWS service client.
        /// </summary>
        /// <param name="credentials">The AWS credentials.</param>
        /// <param name="region">The AWS region.</param>
        /// <typeparam name="T">The interface of the client to create.</typeparam>
        /// <returns>The service client.</returns>
        public static T CreateClient<T>(AWSCredentials credentials, RegionEndpoint region)
            where T : IAmazonService
        {
            var interfaceType = typeof(T);
            Type clientType = interfaceType.IsInterface ? InterfaceTypeToImplementationType(interfaceType) : interfaceType;

            var constructor = clientType.GetConstructor(new Type[] { CredentialsType, RegionType });
            return (T)constructor!.Invoke(new object[] { credentials, region });
        }

        /// <summary>
        /// If they provide an interface, determine the client type name.
        /// </summary>
        /// <param name="interfaceType">The interface for the AWS service client.</param>
        /// <returns>The implementation class type.</returns>
        internal static Type InterfaceTypeToImplementationType(Type interfaceType)
        {
            string interfaceName = interfaceType.Name;
            string implName = interfaceName.Remove(0, 1); // Remove I
            implName += "Client";
            implName = interfaceType.Namespace + "." + implName;
            return interfaceType.Assembly.GetType(implName, true);
        }
    }
}