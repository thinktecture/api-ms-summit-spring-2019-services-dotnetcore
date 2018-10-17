import { LoginModuleConfiguration } from '../app/login/models/login-module-configuration';
import { RealtimeModuleConfiguration } from '../app/realtime/models/realtime-module-configuration';
import { TodoModuleConfiguration } from '../app/todo/models/todo-module-configuration';

export const environment = {
  production: true,
};

export class LoginConfiguration extends LoginModuleConfiguration {
  authorityUrl = 'https://tt-netcore-identity.azurewebsites.net';
  clientId = 'guiclient';
  clientSecret = 'guisecret';
  loginUrl = '/login';
  loginRedirectUrl = '/todo';
}

export class TodoConfiguration extends TodoModuleConfiguration {
  apiUrl = 'https://tt-netcore-api.azurewebsites.net/api';
}

export class RealtimeConfiguration extends RealtimeModuleConfiguration {
  hubUrl = 'https://tt-netcore-push.azurewebsites.net';
}
