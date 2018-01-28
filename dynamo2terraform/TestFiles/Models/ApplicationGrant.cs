using System.Collections.Generic;
using SystemOfEngagement.SharedComponents.DynamoDB;
using SystemOfEngagement.SharedComponents.DynamoDB.OptimisticConcurrency;
using SystemOfEngagement.SharedComponents.DynamoDB.Provisioner;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using NodaTime;

namespace Pushpay.IdentityService.Models
{
	[DynamoDBTable("Services.Identity.ApplicationGrant")]
	public class ApplicationGrant : IVersionedModel
	{
		[DynamoIndex(ProjectionType = ProjectionType.All)]
		public const string ApplicationAndStatusIndex = "ApplicationAndStatus-Index";

		[DynamoIndex(ProjectionType = ProjectionType.All)]
		public const string ApplicationAndScopeIndex = "ApplicationAndScope-Index";

		[DynamoDBHashKey]
		public string Key { get; set; }

		[DynamoDBGlobalSecondaryIndexRangeKey(ApplicationAndScopeIndex)]
		public string ScopeKey { get; set; }

		[DynamoDBGlobalSecondaryIndexHashKey(ApplicationAndStatusIndex, ApplicationAndScopeIndex)]
		public string ApplicationKey { get; set; }

		public List<string> Resources { get; set; }

		[DynamoDBProperty(typeof(InstantConverter))]
		public Instant CreatedAt { get; set; }

		[DynamoDBProperty(typeof(InstantConverter))]
		public Instant UpdatedAt { get; set; }

		[DynamoDBProperty(typeof(InstantConverter))]
		public Instant? DeletedAt { get; set; }

		[DynamoDBGlobalSecondaryIndexRangeKey(ApplicationAndStatusIndex)]
		[DynamoDBEntryType(EntryType = DynamoDBEntryType.Numeric)]
		public ApplicationGrantStatus Status { get; set; }

		public int LockVersion { get; set; }
	}
}

