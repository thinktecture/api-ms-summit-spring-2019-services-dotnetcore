import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanActivateChild, Router, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { LoginModuleConfiguration } from '../models/login-module-configuration';
import { TokenStore } from '../models/token-store';

@Injectable()
export class IsAuthenticatedGuard implements CanActivate, CanActivateChild {
  constructor(
    private readonly tokenStore: TokenStore,
    private readonly router: Router,
    private readonly authenticationConfiguration: LoginModuleConfiguration,
  ) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    return this.isLoggedIn();
  }

  canActivateChild(childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    return this.isLoggedIn();
  }

  private isLoggedIn(): boolean {
    if (this.tokenStore.accessToken) {
      return true;
    }

    this.router.navigate([this.authenticationConfiguration.loginUrl]);
    return false;
  }

}
