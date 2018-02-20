using System.Collections.Generic;

namespace Test.Models
{
	[DynamoDBTable("Services.Person")]
	public class Person
	{
		[DynamoIndex(ProjectionType = ProjectionType.All)]
		public const string NameAndStatusIndex = "NameAndStatus-Index";

		[DynamoIndex(ProjectionType = ProjectionType.All)]
		public const string NameAndAddressIndex = "NameAndAddress-Index";

		[DynamoDBHashKey]
		public string Key { get; set; }

		[DynamoDBGlobalSecondaryIndexRangeKey(NameAndAddressIndex)]
		public string Name { get; set; }

		[DynamoDBGlobalSecondaryIndexHashKey(NameAndStatusIndex, NameAndAddressIndex)]
		public string Address { get; set; }

		public List<string> Items { get; set; }

		[DynamoDBProperty(typeof(InstantConverter))]
		public Instant CreatedAt { get; set; }

		[DynamoDBProperty(typeof(InstantConverter))]
		public Instant UpdatedAt { get; set; }

		[DynamoDBProperty(typeof(InstantConverter))]
		public Instant? DeletedAt { get; set; }

		[DynamoDBGlobalSecondaryIndexRangeKey(NameAndStatusIndex)]
		[DynamoDBEntryType(EntryType = DynamoDBEntryType.Numeric)]
		public PersonStatus Status { get; set; }
	}
}

