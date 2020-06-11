import { Directive, HostListener, OnInit } from '@angular/core';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { ConfirmDialogModel, ConfirmDialogComponent } from '../../../shared/form/confirmdialog/confirmdialog.component';

/**
 * Basis Dialog Input, welcher von allen Dialog-Inputs genutzte Funktionalitäten implementiert
 */
@Directive()
// tslint:disable-next-line: directive-class-suffix
export abstract class BaseDialogInput<T = any> implements OnInit {

    constructor(public dialogRef: MatDialogRef<T>, public dialog: MatDialog) { }

    ngOnInit(): void {
        // Zeige Confirm-Dialog, wenn Benutzer ausserhalb des Dialogs klickt, um den Dialog zu schließen
        this.dialogRef.backdropClick().subscribe(_ => {
            this.confirmDialog();
        });
    }

    // Zeige Confirm-Dialog, wenn Benutzer die ESC-Taste drückt, um den Dialog zu schließen
    @HostListener('document:keydown.escape', ['$event']) onKeydownHandler(event: KeyboardEvent) {
        event.preventDefault();
        event.stopImmediatePropagation();
        event.stopPropagation();
        this.confirmDialog();
    }

    // Zeige Warnung im Browser, wenn Benutzer auf "Reload"-Button klickt
    @HostListener('window:beforeunload', ['$event']) unloadHandler(event: Event) {
        event.returnValue = false;
    }


    /**
     * Funktion zum Aufruf des Confirm-Dialogs
     */
    confirmDialog(): void {
        // Angezeigte Ueberschrift bzw. Nachricht im Confirm-Dialog
        const message = `Wollen Sie den Vorgang wirklich abbrechen?`;
        const dialogData = new ConfirmDialogModel('Warnung', message);

        // Oeffnet den Confirm-Dialog mit definierter Ueberschrift bzw. Nachricht
        const dialogRef = this.dialog.open(ConfirmDialogComponent, {
            maxWidth: '400px',
            data: dialogData,
            disableClose: true
        });

        // Schliesst den Confirm-Dialog
        dialogRef.afterClosed().subscribe(dialogResult => {
            if (dialogResult === true) {
                this.dialogRef.close();
            }
        });
    }
}







