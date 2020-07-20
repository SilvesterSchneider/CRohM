import { OnInit, Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { HistoryElementCreateDto, HistoryElementType, OrganizationService } from 'src/app/shared/api-generated/api-generated';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BaseDialogInput } from '../../shared/form/base-dialog-form/base-dialog.component';
import { JwtService } from 'src/app/shared/jwt.service';

@Component({
    selector: 'app-organizations-add-history',
    templateUrl: './organizations-add-history.component.html',
    styleUrls: ['./organizations-add-history.component.scss']
})

export class OrganizationsAddHistoryComponent extends BaseDialogInput<OrganizationsAddHistoryComponent> implements OnInit {
    public types: string[] = ['E-Mail', 'Telefonanruf', 'Notiz'];
    public oppoSuitsForm: FormGroup;
    constructor(
        public dialogRef: MatDialogRef<OrganizationsAddHistoryComponent>,
        public dialog: MatDialog,
        @Inject(MAT_DIALOG_DATA) public data: number,
        private organizationService: OrganizationService,
        private fb: FormBuilder,
        private jwt: JwtService) {
        super(dialogRef, dialog);
    }

    ngOnInit(): void {
        this.oppoSuitsForm = this.fb.group({
            type: [this.types[0], Validators.required],
            date: ['', Validators.required],
            information: ['', Validators.required],
            comment: ['', Validators.required]
        });
    }

    hasChanged() {
        return !this.oppoSuitsForm.pristine;
    }

    saveHistory() {
        let historyToSave: HistoryElementCreateDto;
        let typeToSave = 0;
        const typeText: string = this.oppoSuitsForm.get('type').value;
        if (typeText === this.types[1]) {
            typeToSave = HistoryElementType.PHONE_CALL;
        } else if (typeText === this.types[2]) {
            typeToSave = HistoryElementType.NOTE;
        }
        historyToSave = {
            date: this.oppoSuitsForm.get('date').value,
            name: this.oppoSuitsForm.get('information').value,
            type: typeToSave,
            comment: this.oppoSuitsForm.get('comment').value,
        };
      //  this.organizationService.postHistoryElement(historyToSave, this.data, this.jwt.getUserId()).subscribe(x => this.dialogRef.close());
    }

    close() {
        super.confirmDialog();
    }
}
