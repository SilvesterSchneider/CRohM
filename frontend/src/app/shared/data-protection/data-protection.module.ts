import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DpUpdatePopupComponent } from './dp-update-popup/dp-update-popup.component';
import { MaterialModule } from '../material.module';
import { MatDialogModule } from '@angular/material/dialog';



@NgModule({
  declarations: [DpUpdatePopupComponent],
  imports: [
    MaterialModule, MatDialogModule,
    CommonModule
  ],
  exports: [DpUpdatePopupComponent],
  entryComponents: [DpUpdatePopupComponent]
})
export class DataProtectionModule { }
