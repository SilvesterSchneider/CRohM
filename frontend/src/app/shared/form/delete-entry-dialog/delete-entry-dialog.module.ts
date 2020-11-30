import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DeleteEntryDialogComponent } from './delete-entry-dialog.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';



@NgModule({
  declarations: [DeleteEntryDialogComponent],
  imports: [
    MatDialogModule,
    CommonModule,
    MatButtonModule,
    TranslateModule
  ],
  entryComponents: [DeleteEntryDialogComponent],
  exports: [DeleteEntryDialogComponent]
})
export class DeleteEntryDialogModule { }
