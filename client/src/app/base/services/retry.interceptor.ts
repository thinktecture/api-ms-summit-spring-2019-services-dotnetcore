import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { delay, filter, retryWhen, take } from 'rxjs/operators';

export class RetryInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      retryWhen(error => error.pipe(
        filter((e: HttpErrorResponse) => e.status !== 404 && e.status !== 401),
        delay(3000),
        take(10))),
    );
  }

}
