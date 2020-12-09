import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-delete-entry-dialog',
  templateUrl: './delete-entry-dialog.component.html',
  styleUrls: ['./delete-entry-dialog.component.scss']
})
export class DeleteEntryDialogComponent implements OnInit {

  constructor(
    public dialogRef: MatDialogRef<DeleteEntryDialogComponent, IDeleteDialogReturnValue>,
    @Inject(MAT_DIALOG_DATA) public data: string) {
    this.dialogRef.backdropClick().subscribe(() => {
      // Close the dialog
      dialogRef.close();
    });
  }

  public ngOnInit(): void {
  }

  public onCancel(): void {
    this.dialogRef.close({ delete: false });
  }

  public onDelete(): void {
    this.dialogRef.close({ delete: true });
  }

}

export interface IDeleteDialogReturnValue {
  delete: boolean;
}
