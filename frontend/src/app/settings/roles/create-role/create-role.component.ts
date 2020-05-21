import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators,  } from '@angular/forms';
import { IPermissionTemp } from '../mock-roles';

@Component({
  selector: 'app-create-role',
  templateUrl: './create-role.component.html',
  styleUrls: ['./create-role.component.scss']
})
export class CreateRoleDialogComponent implements OnInit {
  public roleForm = this.fb.group({
    name: ['', Validators.required],
    permissions: ['']
  });
  constructor(
    public dialogRef: MatDialogRef<CreateRoleDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public permissions: IPermissionTemp[],
    private readonly fb: FormBuilder) {}

 public ngOnInit(): void {
  }
  public  onCancle(): void {
    this.dialogRef.close();
  }
}
