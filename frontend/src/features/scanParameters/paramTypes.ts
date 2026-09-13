import type { DependencyType } from '../../api/types'

export interface AksParamsState {
  subscriptionId: string
  clusterName: string
  environment: string
  tenant: string
  namespace: string
}

export interface AppConfigParamsState {
  application: string
  environment: string
  tenant: string
  namespace: string
  sessionYear: number
}

export interface CharacteristicsParamsState {
  application: string
  environment: string
  tenant: string
  sessionYear: number
  ruleSetId: string
}

export interface DependencyParamsState {
  application: string
  environment: string
  namespace: string
  dependencyType: DependencyType | ''
}
