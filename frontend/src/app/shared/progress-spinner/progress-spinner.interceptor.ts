import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProgressSpinnerOverlayService } from './progress-spinner-overlay.service';
import { finalize } from 'rxjs/operators';

@Injectable()
export class ProgressSpinnerInterceptor implements HttpInterceptor {
  private count = 0;

  constructor(private spinnerService: ProgressSpinnerOverlayService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.spinnerService.showSpinner();
    this.count++;
    return next.handle(request)
      .pipe(finalize(() => {
        this.count--;
        if (this.count === 0) { this.spinnerService.hideSpinner(); }
      })
      );
  }
}
