namespace Suvarna.PMJAY.NHCX.Api.Templates
{
    public static class CoverageEligibilityTemplate
    {
        public const string Json = @"
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
    ""system"": ""https://provider.pmajy.nha.gov.in"",
    ""value"": ""{{BUNDLE_IDENTIFIER}}""
  },
  ""type"": ""collection"",
  ""timestamp"": ""{{BUNDLE_TIMESTAMP}}"",
  ""entry"": [

    {
      ""id"": ""{{CER_FULL_URL}}"",
      ""fullUrl"": ""{{CER_FULL_URL}}"",
      ""resource"": {
        ""resourceType"": ""CoverageEligibilityRequest"",
        ""id"": ""{{CER_RESOURCE_ID}}"",
        ""meta"": {
          ""tag"": [
            {
              ""system"": ""http://terminology.hl7.org/CodeSystem/v3-ObservationValue"",
              ""code"": ""SUBSETTED"",
              ""display"": ""Resource encoded in summary mode""
            }
          ]
        },
        ""identifier"": [
          {
            ""system"": ""https://hcx.pmjay.gov.in/v1/coverageeligibility/check"",
            ""value"": ""{{CASE_ID}}""
          }
        ],
        ""status"": ""active"",
        ""priority"": {
          ""coding"": [
            {
              ""system"": ""http://terminology.hl7.org/CodeSystem/processpriority"",
              ""code"": ""normal"",
              ""display"": ""Normal""
            }
          ]
        },
        ""purpose"": [ ""validation"" ],
        ""patient"": { ""reference"": ""{{PATIENT_REF}}"" },
        ""servicedDate"": ""{{SERVICE_DATE}}"",
        ""created"": ""{{CREATED_DATE}}"",
        ""enterer"": { ""reference"": ""{{PRACTITIONER_REF}}"" },
        ""provider"": { ""reference"": ""{{PROVIDER_REF}}"" },
        ""insurer"": { ""reference"": ""{{INSURER_REF}}"" },
        ""facility"": {
          ""identifier"": {
            ""system"": ""https://nhcx.pmjay.gov.in"",
            ""value"": ""{{FACILITY_CODE}}""
          }
        },
        ""insurance"": [
          {
            ""coverage"": { ""reference"": ""{{COVERAGE_REF}}"" }
          }
        ]
      }
    },

    {
      ""id"": ""{{PATIENT_REF}}"",
      ""fullUrl"": ""{{PATIENT_REF}}"",
      ""resource"": {
        ""resourceType"": ""Patient"",
        ""id"": ""{{PATIENT_ID}}"",
        ""meta"": {
          ""versionId"": ""1"",
          ""lastUpdated"": ""{{BUNDLE_LAST_UPDATED}}"",
          ""tag"": [
            {
              ""system"": ""http://terminology.hl7.org/CodeSystem/v3-ObservationValue"",
              ""code"": ""SUBSETTED"",
              ""display"": ""Resource encoded in summary mode""
            }
          ]
        },
        ""identifier"": [
          {
            ""type"": {
              ""coding"": [
                {
                  ""system"": ""https://nrces.in/ndhm/fhir/r4/CodeSystem/ndhm-identifier-type-code"",
                  ""code"": ""PMJAY"",
                  ""display"": ""Pradhan Mantri Jan Aarogya Yojana (PMJAY) ID""
                }
              ]
            },
            ""system"": ""https://bis.pmjay.gov.in"",
            ""value"": ""{{PMJAY_ID}}""
          }
        ]
      }
    },

    {
      ""id"": ""{{INSURER_REF}}"",
      ""fullUrl"": ""{{INSURER_REF}}"",
      ""resource"": {
        ""resourceType"": ""Organization"",
        ""id"": ""{{INSURER_RESOURCE_ID}}"",
        ""meta"": {
          ""tag"": [
            {
              ""system"": ""http://terminology.hl7.org/CodeSystem/v3-ObservationValue"",
              ""code"": ""SUBSETTED"",
              ""display"": ""Resource encoded in summary mode""
            }
          ]
        },
        ""identifier"": [
          {
            ""type"": {
              ""coding"": [
                {
                  ""system"": ""http://terminology.hl7.org/CodeSystem/v2-0203"",
                  ""code"": ""NIIP"",
                  ""display"": ""National Insurance Payor Identifier (Payor)""
                }
              ]
            },
            ""system"": ""https://facility.abdm.gov.in"",
            ""value"": ""{{INSURER_CODE}}""
          }
        ],
        ""active"": true,
        ""type"": [
          {
            ""coding"": [
              {
                ""system"": ""http://terminology.hl7.org/CodeSystem/organization-type"",
                ""code"": ""pay"",
                ""display"": ""Payer""
              }
            ]
          }
        ],
        ""name"": ""{{INSURER_NAME}}"",
        ""contact"": [
          { ""telecom"": [ { ""system"": ""phone"", ""value"": ""NA"" } ] }
        ]
      }
    },

    {
      ""id"": ""{{PROVIDER_REF}}"",
      ""fullUrl"": ""{{PROVIDER_REF}}"",
      ""resource"": {
        ""resourceType"": ""Organization"",
        ""id"": ""{{PROVIDER_RESOURCE_ID}}"",
        ""meta"": {
          ""tag"": [
            {
              ""system"": ""http://terminology.hl7.org/CodeSystem/v3-ObservationValue"",
              ""code"": ""SUBSETTED"",
              ""display"": ""Resource encoded in summary mode""
            }
          ]
        },
        ""identifier"": [
          {
            ""type"": {
              ""coding"": [
                {
                  ""system"": ""http://terminology.hl7.org/CodeSystem/v2-0203"",
                  ""code"": ""NPI"",
                  ""display"": ""National provider identifier""
                }
              ]
            },
            ""system"": ""https://facility.abdm.gov.in"",
            ""value"": ""{{HOSPITAL_CODE}}""
          }
        ],
        ""active"": true,
        ""type"": [
          {
            ""coding"": [
              {
                ""system"": ""http://terminology.hl7.org/CodeSystem/organization-type"",
                ""code"": ""prov"",
                ""display"": ""Healthcare Provider""
              }
            ]
          }
        ],
        ""name"": ""{{HOSPITAL_NAME}}"",
        ""contact"": [
          { ""telecom"": [ { ""system"": ""phone"", ""value"": ""NA"" } ] }
        ]
      }
    },

    {
      ""id"": ""{{COVERAGE_REF}}"",
      ""fullUrl"": ""{{COVERAGE_REF}}"",
      ""resource"": {
        ""resourceType"": ""Coverage"",
        ""id"": ""{{COVERAGE_RESOURCE_ID}}"",
        ""meta"": {
          ""tag"": [
            {
              ""system"": ""http://terminology.hl7.org/CodeSystem/v3-ObservationValue"",
              ""code"": ""SUBSETTED"",
              ""display"": ""Resource encoded in summary mode""
            }
          ]
        },
        ""identifier"": [
          {
            ""type"": {
              ""coding"": [
                {
                  ""system"": ""http://terminology.hl7.org/CodeSystem/v2-0203"",
                  ""code"": ""NH"",
                  ""display"": ""National Health Plan Identifier""
                }
              ]
            },
            ""system"": ""https://payer.nha.gov.in"",
            ""value"": ""{{POLICY_NUMBER}}""
          }
        ],
        ""status"": ""active"",
        ""beneficiary"": { ""reference"": ""{{PATIENT_REF}}"" },
        ""payor"": [
          { ""reference"": ""{{INSURER_REF}}"" }
        ]
      }
    },

    {
      ""id"": ""{{PRACTITIONER_REF}}"",
      ""fullUrl"": ""{{PRACTITIONER_REF}}"",
      ""resource"": {
        ""resourceType"": ""PractitionerRole"",
        ""id"": ""{{PRACTITIONER_RESOURCE_ID}}"",
        ""meta"": {
          ""tag"": [
            {
              ""system"": ""http://terminology.hl7.org/CodeSystem/v3-ObservationValue"",
              ""code"": ""SUBSETTED"",
              ""display"": ""Resource encoded in summary mode""
            }
          ]
        },
        ""identifier"": [
          {
            ""type"": {
              ""coding"": [
                {
                  ""system"": ""http://snomed.info/sct"",
                  ""code"": ""224533002"",
                  ""display"": ""MO - Medical officer""
                }
              ]
            },
            ""system"": ""https://ump.pmjay.gov.in"",
            ""value"": ""{{FACILITY_CODE}}""
          }
        ]
      }
    }

  ]
}
";
    }
}
