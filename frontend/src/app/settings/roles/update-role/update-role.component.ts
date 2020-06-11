import { Component, OnInit, Inject } from '@angular/core';
import { IRoleTemp, IPermissionTemp } from '../mock-roles';
import { Validators, FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-update-role',
  templateUrl: './update-role.component.html',
  styleUrls: ['./update-role.component.scss']
})
export class UpdateRoleDialogComponent implements OnInit {
  public roleForm = this.fb.group({
    name: ['', Validators.required],
    permissions: ['']
  });
  constructor(
    public dialogRef: MatDialogRef<UpdateRoleDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: IUpdateRoleDialogData,
    private readonly fb: FormBuilder) {}

 public ngOnInit(): void {
  if (this.data != null && this.data !== undefined) {
    this.updateForm();
    }
  }

  public onCancle(): void {
    this.dialogRef.close();
  }

  public onDelete(): void {
    this.dialogRef.close({delete: true});
  }

  private updateForm(): void {
    this.roleForm.patchValue({
      name: this.data.role.name,
      permissions: this.data.role.permissions
    });
  }
}

export interface IUpdateRoleDialogData {
  role: IRoleTemp;
  permissions: IPermissionTemp[];
}
