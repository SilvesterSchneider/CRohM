import { Component, Inject } from '@angular/core';
import { Validators, FormControl, FormBuilder } from '@angular/forms';
import { UsersService, RoleDto, UserDto,
    RoleService } from '../../../shared/api-generated/api-generated';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
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

    oldPermissionGroups: string[] = new Array<string>();

    // Gruppenberechtigungen
    // TODO: Liste aus Backend laden
    permissionGroups: RoleDto[] = new Array<RoleDto>();
    /*[
        { value: 'group_adm', viewValue: 'Administrator' },
        { value: 'group_dat', viewValue: 'Datenschutzbeauftragter' },
        { value: 'group_hig', viewValue: 'Hoch' },
        { value: 'group_med', viewValue: 'Normal' },
        { value: 'group_low', viewValue: 'Eingeschränkt' }
    ]; */
    groupPermission = new FormControl();
    oldId: number;

    constructor(
        private readonly fb: FormBuilder,
        private readonly usersService: UsersService,
        public dialogRef: MatDialogRef<EditUserDialogComponent>,
        public dialog: MatDialog,
        @Inject(MAT_DIALOG_DATA) public data: UserDto,
        private permissionService: RoleService
    ) {
        super(dialogRef, dialog);
        this.oldId = data.id;
        this.userForm.patchValue(this.data);
        this.initPermissions();
    }

    initPermissions() {
        this.usersService.getAllRolesForUser(this.data.id).subscribe(x => {
            this.oldPermissionGroups = x;
            this.permissionService.get().subscribe(y => {
                this.permissionGroups = y;
                this.preselectPermissions();
                }
            );
        });
    }

    preselectPermissions() {
        if (this.oldPermissionGroups != null && this.oldPermissionGroups.length > 0) {
            const textArray: string[] = new Array<string>();
            this.oldPermissionGroups.forEach(x => textArray.push(x));
            this.groupPermission.patchValue(textArray);
        }
    }

    hasChanged() {
        return !this.userForm.pristine;
    }

    /**
     * Funktion zum Speichen des neuen Users und Schliessen des Dialogs
     */
    public editUser() {
        const user: UserDto = this.userForm.value;
        user.id = this.oldId;
        this.usersService.put(user).subscribe(x => this.updatePermissions(user.id));
        // Schliessen des Dialogs
        this.dialogRef.close();
    }

    updatePermissions(id: number): void {
        const groupsToSave: string[] = new Array<string>();
        if (this.groupPermission.value && this.groupPermission.value.length > 0) {
            let idx = 0;
            for (idx = 0; idx < this.groupPermission.value.length; idx++) {
                groupsToSave.push(this.groupPermission.value[idx]);
            }
        }
        this.permissionService.changeUserRoles(this.data.id, groupsToSave).subscribe();
    }
}
