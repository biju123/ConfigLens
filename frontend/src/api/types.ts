// Mirrors backend/src/ConfigLens.Domain and Application DTOs. Enum values
// are the exact PascalCase strings System.Text.Json's JsonStringEnumConverter
// emits; object property names are camelCase (ASP.NET Core's default).

export type ScanCategory =
  | 'AksDeployment'
  | 'ApplicationConfiguration'
  | 'ConfigurationCharacteristics'
  | 'DependencyAccessibility'

export type ScanStatus = 'Pending' | 'Running' | 'Completed' | 'Failed'

export interface ScanMetadata {
  scanId: string
  category: ScanCategory
  startedAtUtc: string
  completedAtUtc: string | null
  status: ScanStatus
  initiatingUser: string
  environment: string | null
  tenant: string | null
  application: string | null
  cluster: string | null
  namespace: string | null
  sessionYear: number | null
}

// --- AKS Deployment ---

export type AksResourceType = 'Deployment' | 'Pod' | 'CronJob' | 'Job' | 'Service' | 'Hpa' | 'KedaScaledObject'

export interface AksResource {
  namespace: string
  appOrService: string
  resourceType: AksResourceType
  resourceName: string
  desiredReplicas: number | null
  runningReplicas: number | null
  readyReplicas: number | null
  cpuRequest: string | null
  cpuLimit: string | null
  memoryRequest: string | null
  memoryLimit: string | null
  containerCount: number | null
  image: string | null
  podStatus: string | null
  restartCount: number | null
  schedule: string | null
  lastScheduleTimeUtc: string | null
  minReplicas: number | null
  maxReplicas: number | null
  currentMetricValue: string | null
  clusterIp: string | null
  ports: string | null
}

export interface AksScanRequest {
  subscriptionId: string
  clusterName: string
  environment: string
  tenant: string
  namespace?: string
}

export interface AksScanResponse {
  metadata: ScanMetadata
  resources: AksResource[]
}

// --- Application Configuration ---

export interface ConfigItem {
  key: string
  value: string | null
  isSensitive: boolean
}

export interface ConfigSection {
  name: string
  items: ConfigItem[]
  subSections: ConfigSection[]
}

export interface ConfigTree {
  rootName: string
  sections: ConfigSection[]
}

export interface AppConfigScanRequest {
  application: string
  environment: string
  tenant: string
  namespace?: string
  sessionYear: number
}

export interface AppConfigScanResponse {
  metadata: ScanMetadata
  tree: ConfigTree
}

// --- Configuration Characteristics ---

export type FindingStatus = 'Pass' | 'Warning' | 'Fail' | 'NotChecked' | 'NotApplicable' | 'Error'
export type FindingSeverity = 'Info' | 'Low' | 'Medium' | 'High' | 'Critical'

export interface Finding {
  findingId: string
  scanId: string
  category: ScanCategory
  application: string
  environment: string
  namespace: string | null
  resource: string
  ruleId: string
  ruleName: string
  severity: FindingSeverity
  status: FindingStatus
  description: string
  expectedCondition: string
  actualCondition: string
  recommendation: string
}

export interface CharacteristicsSummaryCounts {
  pass: number
  warning: number
  fail: number
  notChecked: number
  notApplicable: number
  error: number
}

export interface CharacteristicsScanRequest {
  application: string
  environment: string
  tenant: string
  sessionYear: number
  ruleSetId: string
}

export interface CharacteristicsScanResponse {
  metadata: ScanMetadata
  findings: Finding[]
  summary: CharacteristicsSummaryCounts
}

// --- Dependency Accessibility ---

export type DependencyType = 'AzureSql' | 'AzureTableStorage' | 'AzureBlobStorage' | 'AzureServiceBus' | 'AzureKeyVault' | 'ExternalApi'
export type DependencyStatus = 'ConfigurationMissing' | 'ConfigurationInvalid' | 'Inaccessible' | 'Accessible' | 'Timeout' | 'ValidationError' | 'NotApplicable'

export interface DependencyCheckResult {
  application: string
  dependencyType: DependencyType
  targetDescription: string
  status: DependencyStatus
  failureReason: string | null
  checkedAtUtc: string
  latencyMs: number | null
}

export interface DependencyScanRequest {
  application: string
  environment: string
  namespace: string
  dependencyType?: DependencyType
}

export interface DependencyScanResponse {
  metadata: ScanMetadata
  checks: DependencyCheckResult[]
}

// --- Scan history ---

export type ScanResultRecord =
  | ({ category: 'AksDeployment' } & { resources: AksResource[] })
  | ({ category: 'ApplicationConfiguration' } & { tree: ConfigTree })
  | ({ category: 'ConfigurationCharacteristics' } & {
      findings: Finding[]
      passCount: number
      warningCount: number
      failCount: number
      notCheckedCount: number
      notApplicableCount: number
      errorCount: number
    })
  | ({ category: 'DependencyAccessibility' } & { checks: DependencyCheckResult[] })

export interface ScanRecord {
  metadata: ScanMetadata
  result: ScanResultRecord
}

// --- Comparison ---

export interface AksFieldChange {
  fieldName: string
  oldValue: string | null
  newValue: string | null
}

export interface AksResourceChange {
  namespace: string
  resourceType: AksResourceType
  resourceName: string
  fieldChanges: AksFieldChange[]
}

export interface AksComparisonResult {
  addedResources: AksResource[]
  removedResources: AksResource[]
  changedResources: AksResourceChange[]
}

export interface ConfigEntry {
  path: string
  value: string | null
  isSensitive: boolean
}

export interface ConfigEntryChange {
  path: string
  oldValue: string | null
  newValue: string | null
  isSensitive: boolean
}

export interface AppConfigComparisonResult {
  added: ConfigEntry[]
  removed: ConfigEntry[]
  changed: ConfigEntryChange[]
  unchangedCount: number
}

export interface FindingStatusChange {
  ruleId: string
  resource: string
  oldStatus: FindingStatus
  newStatus: FindingStatus
}

export type RuleChangeType = 'Added' | 'Removed' | 'Modified'

export interface RuleChange {
  ruleId: string
  ruleName: string
  changeType: RuleChangeType
}

export interface CharacteristicsComparisonResult {
  newFailures: Finding[]
  resolvedFailures: Finding[]
  newWarnings: Finding[]
  resolvedWarnings: Finding[]
  statusChanges: FindingStatusChange[]
  ruleChanges: RuleChange[]
}

export interface DependencyStatusChange {
  application: string
  dependencyType: DependencyType
  oldStatus: DependencyStatus
  newStatus: DependencyStatus
}

export interface DependencyComparisonResult {
  statusChanges: DependencyStatusChange[]
  newFailures: DependencyCheckResult[]
  resolvedFailures: DependencyCheckResult[]
}

export type ComparisonResult =
  | AksComparisonResult
  | AppConfigComparisonResult
  | CharacteristicsComparisonResult
  | DependencyComparisonResult

export interface ComparisonResponse {
  currentScanId: string
  baselineScanId: string
  category: ScanCategory
  result: ComparisonResult
}

// --- Reference data ---

export interface RuleSetSummary {
  ruleSetId: string
  name: string
}

export interface ReferenceData {
  subscriptions: string[]
  clusters: string[]
  namespaces: string[]
  environments: string[]
  tenants: string[]
  applications: string[]
  sessionYears: number[]
  ruleSets: RuleSetSummary[]
  dependencyTypes: DependencyType[]
}

// --- Auth ---

export interface LoginRequest {
  username: string
  password: string
}

export interface LoginResponse {
  token: string
  expiresAtUtc: string
  username: string
}
