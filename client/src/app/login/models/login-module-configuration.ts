export abstract class LoginModuleConfiguration {
  abstract authorityUrl: string;
  abstract clientId: string;
  abstract clientSecret: string;
  abstract loginUrl: string;
  abstract loginRedirectUrl: string;
}
