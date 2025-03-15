export interface IOrganisation {
  organisationName?: string;
  town?: string;
  mainTier?: string;
  subTier?: string;
  industry?: string;
  website?: string;
  socialWebsite?: string;
  dateAdded?: string;
}

export interface ITopic {
  name: string;
  description: string;
  type: string;
  id: string;
  createdOn: string;
  disabledOn: string;
  deletedOn: string;
  modifiedOn: string;
  version: string;
}

export class Configuration {
  id?: string;
  name?: string;
  description?: string;
  topics?: ITopic[];
}

export interface IFilterParameters {
  cities: string[];
  industries?: string[];
  mainTier?: string[];
  subTier?: string[];
  versions?: DataVersion[];
  organizationCount: number;
}

export class PageRequest {
  organisationName?: string | null;
  town: string = '';
  pageNumber: number = 1;
  rowsPerPage: number = 20;
  industry?: string | null;
  subTier?: string | null;
  mainTier?: string | null;
  version?: string | null;
}

export class AuditLog {
  email?: string;
  loggedAt?: string;
  iPAddress?: string;
  agentInformation?: string;
  location?: string;
}

export class DataVersion {
  version?: string;
  title?: string;
}

export class TopicNotification {
  runId?: string;
  currentTopic?: any;
}
