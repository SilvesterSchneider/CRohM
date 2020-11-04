import { OnInit, Component } from '@angular/core';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { HistoryElementCreateDto, HistoryElementType } from 'src/app/shared/api-generated/api-generated';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BaseDialogInput } from '../form/base-dialog-form/base-dialog.component';

@Component({
    selector: 'app-add-history',
    templateUrl: './add-history.component.html',
    styleUrls: ['./add-history.component.scss']
})

export class AddHistoryComponent extends BaseDialogInput<AddHistoryComponent> implements OnInit {
    public types: string[] = ['E-Mail', 'Telefonanruf', 'Notiz', 'Besuch'];
    public oppoSuitsForm: FormGroup;
    constructor(
        public dialogRef: MatDialogRef<AddHistoryComponent>,
        public dialog: MatDialog,
        private fb: FormBuilder) {
        super(dialogRef, dialog);
        this.dialogRef.backdropClick().subscribe(() => {
			// Close the dialog
			dialogRef.close();
		});
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

    getObject(): HistoryElementCreateDto {
        let historyToSave: HistoryElementCreateDto;
        let typeToSave = HistoryElementType.MAIL;
        const typeText: string = this.oppoSuitsForm.get('type').value;
        if (typeText === this.types[1]) {
            typeToSave = HistoryElementType.PHONE_CALL;
        } else if (typeText === this.types[2]) {
            typeToSave = HistoryElementType.NOTE;
        } else if (typeText === this.types[3]) {
            typeToSave = HistoryElementType.VISIT;
        }
        historyToSave = {
            date: this.oppoSuitsForm.get('date').value,
            name: this.oppoSuitsForm.get('information').value,
            type: typeToSave,
            comment: this.oppoSuitsForm.get('comment').value,
        };
        return historyToSave;
    }

    public onCancel(): void {
        super.confirmDialog();
    }
}
