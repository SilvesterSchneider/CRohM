import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AuthService, UserDto } from 'src/app/shared/api-generated/api-generated';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
    selector: 'app-change-password-dialog',
    templateUrl: './change-password-dialog.component.html',
    styleUrls: ['./change-password-dialog.component.scss']
  })

  export class ChangePasswordComponent implements OnInit {
    formGroup: FormGroup;
    text: string;
    errorText: string;

    constructor(
      public dialogRef: MatDialogRef<ChangePasswordComponent>,
      @Inject(MAT_DIALOG_DATA) public data: UserDto,
      private authService: AuthService,
      private fb: FormBuilder,
    ) {
        this.text = this.data.firstName + ' ' + this.data.lastName;
    }

    ngOnInit(): void {
        this.formGroup = this.createForm();
    }

    private createForm(): FormGroup {
        return this.fb.group({
          password: ['', Validators.required],
          error: ['']
        });
    }

    public savePassword() {
        this.authService.updatePassword(this.data.id, this.formGroup.get('password').value).subscribe((x) => {
          if (x) {
            this.dialogRef.close();
          }
        }, error => {
          if (error == null) {
            this.dialogRef.close();
          } else {
            this.errorText = error;
            this.formGroup.get('error').setValue(error);
          }
        });
    }
}
