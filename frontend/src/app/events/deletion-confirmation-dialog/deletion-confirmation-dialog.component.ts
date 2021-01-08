import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-deletion-confirmation-dialog',
  templateUrl: './deletion-confirmation-dialog.component.html',
  styleUrls: ['./deletion-confirmation-dialog.component.scss']
})
/// <summary>
/// RAM: 100%
/// </summary>
export class DeletionConfirmationDialogComponent implements OnInit {
  constructor(
    public dialogRef: MatDialogRef<DeletionConfirmationDialogComponent>) {
        this.dialogRef.backdropClick().subscribe(() => {
        // Close the dialog
        dialogRef.close();
      });
    }

  public ngOnInit(): void {
  }

  public onCancel(): void {
    this.dialogRef.close({delete: false});
  }

  public onDelete(): void {
    this.dialogRef.close({delete: true});
  }
}
