import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {
  constructor(private _snackBar: MatSnackBar) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request)
      .pipe(catchError((error: HttpErrorResponse) => {
        console.error(error);
        this._snackBar.open(`Es ist ein Fehler aufgetreten: ${error.status} - ${error.statusText}`, undefined, {
          duration: 4000,
          panelClass: ['mat-toolbar', 'mat-warn'],
        });
        return throwError(error);
      }));
  }
}
