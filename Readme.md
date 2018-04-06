# Dynamo2Terraform
Dynamo2Terraform is a command line tool for transforming DynamoDB models into an output format specified by a Liquid template.
The tool currently supports models written in C#.

## Usage

```
▶ dynamo2terraform [options]

Options:
  -?|-h|--help          Show help information
  -i|--input <path>     The path to the input C# DynamoDB Model decorated with DynamoDBAttributes for parsing
  -t|--template <path>  The path to the liquid template to be used for generating the output
```

## Example

Using the `Person.cs` model and the `TerraformTemplate.liquid` template provided in the `Samples` folder

### Input:
```
▶ dynamo2terraform -i Samples/Person.cs -t Samples/TerraformTemplate.liquid 
```

### Output:
```
# Services.Person table

resource "aws_dynamodb_table" "table_name" {
  name           = "${var.environment}_Services.Person"
  read_capacity  = 1
  write_capacity = 1
  hash_key       = "Key"

        attribute {
                name = "Key"
                type = "S"
        }
        attribute {
                name = "Address"
                type = "S"
        }
        attribute {
                name = "Status"
                type = ""
        }
        attribute {
                name = "Name"
                type = "S"
        }
        global_secondary_index {
                hash_key        = "Address"
                range_key       = "Status"
                name            = "NameAndStatus-Index"
                projection_type = "ALL"
                read_capacity   = 1
                write_capacity  = 1
        }
        global_secondary_index {
                hash_key        = "Address"
                range_key       = "Name"
                name            = "NameAndAddress-Index"
                projection_type = "ALL"
                read_capacity   = 1
                write_capacity  = 1
        }

  tags {
    App         = "se"
    Environment = "${var.environment}"
  }
}
```
