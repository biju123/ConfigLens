import type {
  AksScanResponse,
  AppConfigScanResponse,
  CharacteristicsScanResponse,
  DependencyScanResponse,
  ReferenceData,
  ScanRecord,
} from '../api/types'

export const referenceDataFixture: ReferenceData = {
  subscriptions: ['sub-assessor-prod-01'],
  clusters: ['aks-assessor-prod-eastus'],
  namespaces: ['assessor-prod'],
  environments: ['Production', 'Staging'],
  tenants: ['ContosoCounty'],
  applications: ['AssessorApi'],
  sessionYears: [2025, 2026],
  ruleSets: [{ ruleSetId: 'default', name: 'Default Rule Set' }],
  dependencyTypes: ['AzureSql', 'AzureTableStorage', 'AzureBlobStorage', 'AzureServiceBus', 'AzureKeyVault', 'ExternalApi'],
}

export const aksScanResponseFixture: AksScanResponse = {
  metadata: {
    scanId: 'SCAN-20260912-00001',
    category: 'AksDeployment',
    startedAtUtc: '2026-09-12T10:00:00Z',
    completedAtUtc: '2026-09-12T10:00:01Z',
    status: 'Completed',
    initiatingUser: 'user',
    environment: 'Production',
    tenant: 'ContosoCounty',
    application: null,
    cluster: 'aks-assessor-prod-eastus',
    namespace: 'assessor-prod',
    sessionYear: null,
  },
  resources: [
    {
      namespace: 'assessor-prod',
      appOrService: 'assessor-api',
      resourceType: 'Deployment',
      resourceName: 'assessor-api',
      desiredReplicas: 3,
      runningReplicas: 3,
      readyReplicas: 3,
      cpuRequest: '250m',
      cpuLimit: '500m',
      memoryRequest: '256Mi',
      memoryLimit: '512Mi',
      containerCount: 1,
      image: 'assessorregistry.azurecr.io/assessor-api:2026.3.1',
      podStatus: null,
      restartCount: null,
      schedule: null,
      lastScheduleTimeUtc: null,
      minReplicas: null,
      maxReplicas: null,
      currentMetricValue: null,
      clusterIp: null,
      ports: null,
    },
  ],
}

export const appConfigScanResponseFixture: AppConfigScanResponse = {
  metadata: { ...aksScanResponseFixture.metadata, category: 'ApplicationConfiguration', application: 'AssessorApi', sessionYear: 2026 },
  tree: {
    rootName: 'AssessorApi',
    sections: [
      {
        name: 'Database',
        items: [
          { key: 'ConnectionString', value: '********', isSensitive: true },
          { key: 'RetryCount', value: '3', isSensitive: false },
        ],
        subSections: [],
      },
    ],
  },
}

export const characteristicsScanResponseFixture: CharacteristicsScanResponse = {
  metadata: { ...aksScanResponseFixture.metadata, category: 'ConfigurationCharacteristics', application: 'AssessorApi', sessionYear: 2026 },
  findings: [
    {
      findingId: 'SCAN-20260912-00001:r1',
      scanId: 'SCAN-20260912-00001',
      category: 'ConfigurationCharacteristics',
      application: 'AssessorApi',
      environment: 'Production',
      namespace: null,
      resource: 'Database.RetryCount',
      ruleId: 'r1',
      ruleName: 'Database retry count is required',
      severity: 'High',
      status: 'Fail',
      description: 'Database retry count must be configured.',
      expectedCondition: 'Database.RetryCount is configured',
      actualCondition: 'Database.RetryCount is not configured.',
      recommendation: 'Set Database:RetryCount.',
    },
    {
      findingId: 'SCAN-20260912-00001:r2',
      scanId: 'SCAN-20260912-00001',
      category: 'ConfigurationCharacteristics',
      application: 'AssessorApi',
      environment: 'Production',
      namespace: null,
      resource: 'Database.ConnectionString',
      ruleId: 'r2',
      ruleName: 'Database connection string is required',
      severity: 'Critical',
      status: 'Pass',
      description: 'Database connection string must be configured.',
      expectedCondition: 'Database.ConnectionString is configured',
      actualCondition: 'Database.ConnectionString is configured.',
      recommendation: '',
    },
  ],
  summary: { pass: 1, warning: 0, fail: 1, notChecked: 0, notApplicable: 0, error: 0 },
}

export const dependencyScanResponseFixture: DependencyScanResponse = {
  metadata: { ...aksScanResponseFixture.metadata, category: 'DependencyAccessibility', application: 'AssessorApi' },
  checks: [
    {
      application: 'AssessorApi',
      dependencyType: 'AzureSql',
      targetDescription: 'sql-assessor-prod/Assessor',
      status: 'Accessible',
      failureReason: null,
      checkedAtUtc: '2026-09-12T10:00:00Z',
      latencyMs: 42,
    },
    {
      application: 'AssessorApi',
      dependencyType: 'AzureKeyVault',
      targetDescription: 'kv-assessor-prod',
      status: 'ConfigurationInvalid',
      failureReason: 'Key Vault URI is not a valid HTTPS URI.',
      checkedAtUtc: '2026-09-12T10:00:00Z',
      latencyMs: null,
    },
  ],
}

export const aksScanRecordFixture: ScanRecord = {
  metadata: aksScanResponseFixture.metadata,
  result: { category: 'AksDeployment', resources: aksScanResponseFixture.resources },
}

export const aksScanRecordFixture2: ScanRecord = {
  metadata: { ...aksScanResponseFixture.metadata, scanId: 'SCAN-20260911-00001', startedAtUtc: '2026-09-11T10:00:00Z' },
  result: { category: 'AksDeployment', resources: [] },
}
