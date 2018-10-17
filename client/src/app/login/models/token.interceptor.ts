import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TokenStore } from './token-store';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  constructor(private readonly tokenStore: TokenStore) {
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    let httpRequest = req;

    if (this.tokenStore.accessToken) {
      httpRequest = req.clone({ setHeaders: { Authorization: `Bearer ${this.tokenStore.accessToken}` } });
    }

    return next.handle(httpRequest);
  }

}
