namespace Suvarna.DGMS.Api.Templates
{
    public static class InsurancePlanTemplate
    {
        public static readonly string Json = @"
{
  ""resourceType"": ""Bundle"",
  ""id"": ""{{BUNDLE_ID}}"",
  ""meta"": {
    ""lastUpdated"": ""{{BUNDLE_LAST_UPDATED}}"",
    ""tag"": [
      {
        ""system"": ""http://terminology.hl7.org/CodeSystem/v3-ObservationValue"",
        ""code"": ""SUBSETTED"",
        ""display"": ""Resource encoded in summary mode""
      }
    ]
  },
  ""identifier"": {
    ""system"": ""https://payer.pmjay.nha.gov.in"",
    ""value"": ""{{BUNDLE_IDENTIFIER}}""
  },
  ""type"": ""collection"",
  ""entry"": [
    {
      ""id"": ""{{TASK_FULL_URL}}"",
      ""fullUrl"": ""{{TASK_FULL_URL}}"",
      ""resource"": {
        ""resourceType"": ""Task"",
        ""id"": ""{{TASK_RESOURCE_ID}}"",
        ""meta"": {
          ""tag"": [
            {
              ""system"": ""http://terminology.hl7.org/CodeSystem/v3-ObservationValue"",
              ""code"": ""SUBSETTED"",
              ""display"": ""Resource encoded in summary mode""
            }
          ]
        },
        ""status"": ""requested"",
        ""intent"": ""original-order"",
        ""code"": {
          ""coding"": [
            {
              ""system"": ""http://terminology.hl7.org/CodeSystem/financialtaskcode"",
              ""code"": ""poll"",
              ""display"": ""Poll""
            }
          ]
        },
        ""input"": [
          {
            ""type"": {
              ""coding"": [
                {
                  ""system"": ""https://nrces.in/ndhm/fhir/r4/CodeSystem/ndhm-task-input-type-code"",
                  ""code"": ""policyNumber"",
                  ""display"": ""PolicyNumber""
                }
              ]
            },
            ""valueString"": ""{{POLICY_NUMBER}}""
          },
          {
            ""type"": {
              ""coding"": [
                {
                  ""system"": ""https://nrces.in/ndhm/fhir/r4/CodeSystem/ndhm-task-input-type-code"",
                  ""code"": ""providerId"",
                  ""display"": ""Provider ID""
                }
              ]
            },
            ""valueString"": ""{{PROVIDER_ID}}""
          }
        ]
      }
    }
  ]
}";
    }
}
