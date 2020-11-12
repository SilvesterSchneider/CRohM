import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Component, OnInit, Inject } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';



/**
 * Diese Komponente dient zur Erzeugung von einheitlichen Bestaetigungs-Dialogen.
 * Ueberschrift und Nachricht koennen uebergeben werden.
 * Rueckgabe ist true bei Klick auf 'Ja' oder false bei Klick auf 'Nein'.
 */
@Component({
  selector: 'app-confirmdialog',
  templateUrl: './confirmdialog.component.html',
  styleUrls: ['./confirmdialog.component.scss']
})
export class ConfirmDialogComponent implements OnInit {
  title: string;
  message: string;

  constructor(
    public dialogRef: MatDialogRef<ConfirmDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ConfirmDialogModel, private translate: TranslateService) {
    // Update view with given values
    this.title = translate.instant(data.title);
    this.message = translate.instant(data.message);
  }

  ngOnInit() {
  }

  onConfirm(): void {
    // Close the dialog, return true
    this.dialogRef.close(true);
  }

  onDismiss(): void {
    // Close the dialog, return false
    this.dialogRef.close(false);
  }
}

/**
 * Class to represent confirm dialog model.
 *
 * It has been kept here to keep it as part of shared component.
 */
export class ConfirmDialogModel {

  constructor(public title: string, public message: string) {
  }
}
