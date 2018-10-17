import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { LoginModuleConfiguration } from '../models/login-module-configuration';
import { TokenStore } from '../models/token-store';

const ACCESS_TOKEN_STORAGE_KEY = 'todo:accesstoken';

@Injectable()
export class AuthenticationService {
  public sessionActivated$: Observable<void>;
  public sessionDeactivated$: Observable<void>;
  private sessionActivated = new ReplaySubject<void>(1);
  private sessionDeactivated = new ReplaySubject<void>(1);

  constructor(
    private readonly tokenStore: TokenStore,
    private readonly http: HttpClient,
    private readonly authenticationConfiguration: LoginModuleConfiguration,
  ) {
    this.sessionActivated$ = this.sessionActivated.asObservable();
    this.sessionDeactivated$ = this.sessionDeactivated.asObservable();
  }

  activateSession() {
    this.tokenStore.accessToken = localStorage.getItem(ACCESS_TOKEN_STORAGE_KEY);

    if (this.tokenStore.accessToken) {
      this.sessionActivated.next(void 0);
    }
  }

  login(username: string, password: string): Observable<void> {
    const formData = new FormData();
    formData.append('grant_type', 'password');
    formData.append('username', username);
    formData.append('password', password);
    formData.append('client_id', this.authenticationConfiguration.clientId);
    formData.append('client_secret', this.authenticationConfiguration.clientSecret);
    return this.http.post(`${this.authenticationConfiguration.authorityUrl}/connect/token`, formData, { observe: 'body' })
      .pipe(map(this.handleResponse.bind(this)));
  }

  logout() {
    this.tokenStore.accessToken = void 0;
    localStorage.removeItem(ACCESS_TOKEN_STORAGE_KEY);

    this.sessionDeactivated.next(void 0);
  }

  private handleResponse(body: any): void {
    if (!body || !body.access_token) {
      throw new Error('Invalid response from IdentityServer');
    }

    this.tokenStore.accessToken = body.access_token;
    localStorage.setItem(ACCESS_TOKEN_STORAGE_KEY, this.tokenStore.accessToken);
    this.sessionActivated.next(void 0);
  }
}
