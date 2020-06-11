import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DeleteEntryDialogComponent } from './delete-entry-dialog.component';
import { MatDialogModule } from '@angular/material/dialog';
import {MatButtonModule} from '@angular/material/button';



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
