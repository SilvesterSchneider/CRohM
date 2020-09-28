import { CommonModule } from '@angular/common';
import { DeleteEntryDialogComponent } from './delete-entry-dialog.component';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { NgModule } from '@angular/core';

@NgModule({
  declarations: [DeleteEntryDialogComponent],
  imports: [
    MatDialogModule,
    CommonModule,
    MatButtonModule
  ],
  entryComponents: [DeleteEntryDialogComponent],
  exports: [DeleteEntryDialogComponent]
})
export class DeleteEntryDialogModule { }
