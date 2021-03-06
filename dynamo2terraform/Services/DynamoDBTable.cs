﻿using System.Collections.Generic;

namespace dynamo2terraform.Services
{
	public class DynamoDbTable
	{
		public DynamoDbAttribute HashKey { get; set; }
		public DynamoDbAttribute RangeKey { get; set; }
		public List<DynamoDbGlobalSecondaryIndex> GlobalSecondaryIndexes { get; set; }
		public string Name { get; set; }
		public List<DynamoDbAttribute> Attributes { get; set; }
	}

	public class DynamoDbAttribute
	{
		public string Name { get; set; }
		public string Type { get; set; }
	}

	public class DynamoDbGlobalSecondaryIndex
	{
		public DynamoDbAttribute HashKey { get; set; }
		public DynamoDbAttribute RangeKey { get; set; }
		public string Name { get; set; }
		public string ProjectionType { get; set; }
	}
}
