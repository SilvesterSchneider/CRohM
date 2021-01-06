import { Component } from '@angular/core';
import { Validators, FormControl, FormBuilder } from '@angular/forms';
import { UsersService, RoleDto, RoleService } from '../../../shared/api-generated/api-generated';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { BaseDialogInput } from '../../../shared/form/base-dialog-form/base-dialog.component';
import { RolesTranslationService } from '../../roles/roles-translation.service';
import { MatSnackBar } from '@angular/material/snack-bar';

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
    permissionGroups: RoleDto[] = new Array<RoleDto>();

    constructor(
        private readonly fb: FormBuilder,
        private readonly usersService: UsersService,
        public dialogRef: MatDialogRef<AddUserDialogComponent>,
        public dialog: MatDialog,
        private permissionService: RoleService,
        private snackBar: MatSnackBar
    ) {
        super(dialogRef, dialog);
        this.initPermissions();
    }

    initPermissions() {
        this.permissionService.get().subscribe(x => this.permissionGroups = x);
    }

    getRoleLabel(role: string){
        return RolesTranslationService.mapRole(role).label;
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
        }, error => {
            this.snackBar.open(error, undefined, {
                duration: 4000,
                panelClass: ['mat-toolbar', 'mat-warn'],
              });
        });
        // Schliessen des Dialogs
        this.dialogRef.close();
    }

    addPermissionsForUser(id: number) {
        if (this.groupPermission.value && this.groupPermission.value.length > 0) {
            let idx = 0;
            const groupToAdd: string[] = new Array<string>();
            for (idx = 0; idx < this.groupPermission.value.length; idx++) {
                if (this.permissionGroups == null || this.permissionGroups.length === 0) {
                    break;
                }
                const group: RoleDto = this.permissionGroups.find(y => y.name === this.groupPermission.value[idx]);
                if (group) {
                    groupToAdd.push(group.name);
                }
            }
            if (groupToAdd.length > 0) {
                this.permissionService.changeUserRoles(id, groupToAdd).subscribe();
            }
        }
    }
}







