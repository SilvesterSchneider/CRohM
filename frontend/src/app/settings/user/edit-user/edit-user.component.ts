import { Component } from '@angular/core';
import { Validators, FormControl, FormBuilder } from '@angular/forms';
import { UsersService } from '../../../shared/api-generated/api-generated';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { Permission } from '../user.model';
import { BaseDialogInput } from '../../../shared/form/base-dialog-form/base-dialog.component';


/**
 * Komponente fuer den Modal-Dialog zum Editieren eines Nutzers
 */
@Component({
    selector: 'app-edit-user',
    templateUrl: './edit-user.component.html',
    styleUrls: ['./edit-user.component.scss']
})
export class EditUserDialogComponent extends BaseDialogInput<EditUserDialogComponent> {

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
    ) {
        super(dialogRef, dialog);
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
}
