import { Overlay, OverlayRef } from '@angular/cdk/overlay';
import { ComponentPortal } from '@angular/cdk/portal';
import { Injectable } from '@angular/core';
import { MatSpinner } from '@angular/material/progress-spinner';

@Injectable({
  providedIn: 'root',
})
export class ProgressSpinnerOverlayService {
  private spinnerRef: OverlayRef = this.cdkSpinnerCreate();

  constructor(private overlay: Overlay) { }

  private cdkSpinnerCreate() {
    return this.overlay.create({
      hasBackdrop: true,
      positionStrategy: this.overlay.position()
        .global()
        .centerHorizontally()
        .centerVertically(),
    });
  }
  showSpinner() {
    if (!this.spinnerRef.hasAttached()) {
      const componentRef = this.spinnerRef.attach(new ComponentPortal(MatSpinner));
      componentRef.instance.diameter = 60;
    }
  }
  hideSpinner() {
    if (this.spinnerRef.hasAttached()) {
      this.spinnerRef.detach();
    }
  }
}
