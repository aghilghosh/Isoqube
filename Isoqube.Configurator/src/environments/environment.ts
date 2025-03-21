// This file can be replaced during build by using the `fileReplacements` array.
// `ng build` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  APP_ECOSYSTEM: 'Isoqube',
  APP_NAME: 'Isoqube Configurator',
  APP_VERSION: '1.0.0.0',
  API_ENDPOINT: 'http://localhost:5000',
  API_DEFAULT_INGESTION: 'http://localhost:5001',
  API_WINDOWS_POOL_ENDPOINT: 'http://localhost:5002',
  API_LINUX_POOL_ENDPOINT: 'http://localhost:5003',
  API_DEFAULT_DEPLOYMENT: 'http://localhost:5004'
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.
