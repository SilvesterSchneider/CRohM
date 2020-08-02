import { Component } from '@angular/core';
import { Validators, FormControl, FormBuilder } from '@angular/forms';
import { UsersService, PermissionsService, PermissionGroupDto, UserPermissionsService } from '../../../shared/api-generated/api-generated';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { BaseDialogInput } from '../../../shared/form/base-dialog-form/base-dialog.component';

/**
 * Komponente fuer den Modal-Dialog zum Hinzufuegen eines Nutzers
 */
@Component({
    selector: 'app-add-user',
    templateUrl: './add-user.component.html',
    styleUrls: ['./add-user.component.scss']
})
export class AddUserDialogComponent extends BaseDialogInput<AddUserDialogComponent> {
    userForm = this.fb.group({
        email: ['', [Validators.email, Validators.required]],
        firstName: ['', Validators.required],
        lastName: ['', Validators.required]
    });

    // Gruppenberechtigungen
    // TODO: Liste aus Backend laden
  /*  groupPermissionList: Permission[] = [
        { value: 'group_adm', viewValue: 'Administrator' },
        { value: 'group_dat', viewValue: 'Datenschutzbeauftragter' },
        { value: 'group_hig', viewValue: 'Hoch' },
        { value: 'group_med', viewValue: 'Normal' },
        { value: 'group_low', viewValue: 'Eingeschränkt' }
    ]; */
    groupPermission = new FormControl();
    permissionGroups: PermissionGroupDto[] = new Array<PermissionGroupDto>();

    constructor(
        private readonly fb: FormBuilder,
        private readonly usersService: UsersService,
        public dialogRef: MatDialogRef<AddUserDialogComponent>,
        public dialog: MatDialog,
        private permissionService: PermissionsService,
        private userPermissions: UserPermissionsService
    ) {
        super(dialogRef, dialog);
        this.initPermissions();
    }

    initPermissions() {
        this.permissionService.getAllPermissionGroups().subscribe(x => this.permissionGroups = x);
    }

    hasChanged() {
        return !this.userForm.pristine;
    }

    /**
     * Funktion zum Speichen des neuen Users und Schliessen des Dialogs
     */
    public addUser() {
        this.usersService.post(this.userForm.value).subscribe(user => {
            this.addPermissionsForUser(user.id);
        });
        // Schliessen des Dialogs
        this.dialogRef.close();
    }

    addPermissionsForUser(id: number) {
        if (this.groupPermission.value && this.groupPermission.value.length > 0) {
            let idx = 0;
            for (idx = 0; idx < this.groupPermission.value.length; idx++) {
                if (this.permissionGroups == null || this.permissionGroups.length === 0) {
                    break;
                }
                const group: PermissionGroupDto = this.permissionGroups.find(y => y.name === this.groupPermission.value[idx]);
                if (group) {
                    this.userPermissions.addPermissionsByUserId(id, group.id).subscribe();
                }
            }
        }
    }
}







