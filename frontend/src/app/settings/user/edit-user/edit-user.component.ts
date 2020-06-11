import { Component } from '@angular/core';
import { Validators, FormControl, FormBuilder } from '@angular/forms';
import { UsersService } from '../../../shared/api-generated/api-generated';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { ConfirmDialogModel, ConfirmDialogComponent } from '../../../confirmdialog/confirmdialog.component';
import { Permission } from '../user.model';


/**
 * Komponente fuer den Modal-Dialog zum Editieren eines Nutzers
 */
@Component({
    selector: 'app-edit-user',
    templateUrl: './edit-user.component.html',
    styleUrls: ['./edit-user.component.scss']
})
export class EditUserDialogComponent {

    userForm = this.fb.group({
        email: ['', [Validators.email, Validators.required]],
        firstName: ['', Validators.required],
        lastName: ['', Validators.required]
    });

    // Gruppenberechtigungen
    // TODO: Liste aus Backend laden
    groupPermissionList: Permission[] = [
        { value: 'group_adm', viewValue: 'Administrator' },
        { value: 'group_dat', viewValue: 'Datenschutzbeauftragter' },
        { value: 'group_hig', viewValue: 'Hoch' },
        { value: 'group_med', viewValue: 'Normal' },
        { value: 'group_low', viewValue: 'Eingeschränkt' }
    ];
    groupPermissionDefault = ['group_low'];
    groupPermission = new FormControl(this.groupPermissionDefault);


    constructor(
        private readonly fb: FormBuilder,
        private readonly usersService: UsersService,
        public dialogRef: MatDialogRef<EditUserDialogComponent>,
        public dialog: MatDialog,
    ) { }


    /**
     * Funktion zum Schliessen des Dialogs
     */
    abortDialog(): void {
        this.dialogRef.close();
    }

    /**
     * Funktion zum Speichen des neuen Users und Schliessen des Dialogs
     */
    public editUser() {
        this.usersService.post(this.userForm.value).subscribe(user => {
        });

        // TODO: Speichern/Uebermitteln der Gruppenbrerchtigungen
        // Ausgabe der Daten auf der Konsole zum Debuggen
        console.log(this.groupPermission.value);

        // Schliessen des Dialogs
        this.dialogRef.close();
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
            data: dialogData
        });

        // Schliesst den Confirm-Dialog
        dialogRef.afterClosed().subscribe(dialogResult => {
            // Wenn Nein angeklickt wurde passiert nichts.
            // Wenn 'Ja' angeklickt wurde, dann wird die abortDialog-Funktion
            // des drunterliegenden Dialogs (hier also Dialog zum Hinzufuegen
            // eines Nutzers) geschlossen.
            if (dialogResult === true) {
                this.abortDialog();
            }
        });
    }
}
