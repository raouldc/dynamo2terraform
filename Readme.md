# Dynamo2Terraform
Dynamo2Terraform is a command line tool for transforming DynamoDB models into an output format specified by a Liquid template.
The tool currently supports models written in C#.

# Usage

```▶ dynamo2terraform [options]

Options:
  -?|-h|--help          Show help information
  -i|--input <path>     The path to the input C# DynamoDB Model decorated with DynamoDBAttributes for parsing
  -t|--template <path>  The path to the liquid template to be used for generating the output```
