import { LoginModuleConfiguration } from '../app/login/models/login-module-configuration';
import { RealtimeModuleConfiguration } from '../app/realtime/models/realtime-module-configuration';
import { TodoModuleConfiguration } from '../app/todo/models/todo-module-configuration';

export const environment = {
  production: false,
};

export class LoginConfiguration extends LoginModuleConfiguration {
  authorityUrl = 'http://localhost:5000';
  clientId = 'guiclient';
  clientSecret = 'guisecret';
  loginUrl = '/login';
  loginRedirectUrl = '/todo';
}

export class TodoConfiguration extends TodoModuleConfiguration {
  apiUrl = 'http://localhost:5001/api';
}

export class RealtimeConfiguration extends RealtimeModuleConfiguration {
  hubUrl = 'http://localhost:5002';
}
