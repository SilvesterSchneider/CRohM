import { OnInit, Component, Inject } from '@angular/core';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { HistoryElementCreateDto, HistoryElementType } from 'src/app/shared/api-generated/api-generated';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { BaseDialogInput } from '../form/base-dialog-form/base-dialog.component';

@Component({
    selector: 'app-add-history',
    templateUrl: './add-history.component.html',
    styleUrls: ['./add-history.component.scss']
})
/// <summary>
/// RAM: 90%
/// </summary>
export class AddHistoryComponent extends BaseDialogInput<AddHistoryComponent> implements OnInit {
    public types = [
        {
            translate: 'contact.mail',
            type: HistoryElementType.MAIL
        },
        {
            translate: 'common.phoneCall',
            type: HistoryElementType.PHONE_CALL
        },
        {
            translate: 'common.note',
            type: HistoryElementType.NOTE
        },
        {
            translate: 'common.visit',
            type: HistoryElementType.VISIT
        }];
    public oppoSuitsForm: FormGroup;
    constructor(
        public dialogRef: MatDialogRef<AddHistoryComponent>,
        public dialog: MatDialog,
        @Inject(MAT_DIALOG_DATA) public data: HistoryDialogModel,
        private fb: FormBuilder) {
        super(dialogRef, dialog);
        this.dialogRef.backdropClick().subscribe(() => {
            // Close the dialog
            dialogRef.close();
        });
    }

    ngOnInit(): void {
        const dateToInsert = this.getDateOfValueAsText(new Date(Date.now()));
        this.oppoSuitsForm = this.fb.group({
            type: [this.types[0], Validators.required],
            date: [new FormControl(new Date(dateToInsert)), Validators.required],
            information: ['', Validators.required],
            comment: ['', Validators.required]
        });

        if (this.data != null && this.data.type != null) {
            this.oppoSuitsForm.get('type').setValue(this.data.type);
            this.oppoSuitsForm.get('comment').setValue(this.data.value);
        }

        this.oppoSuitsForm.get('date').setValue(dateToInsert);

    }

    hasChanged() {
        return !this.oppoSuitsForm.pristine;
    }

    getObject(): HistoryElementCreateDto {
        let historyToSave: HistoryElementCreateDto;
        const date = new Date(this.oppoSuitsForm.get('date').value);
        historyToSave = {
            date: this.getDateOfValueAsText(date),
            name: this.oppoSuitsForm.get('information').value,
            type: this.oppoSuitsForm.get('type').value,
            comment: this.oppoSuitsForm.get('comment').value,
        };
        return historyToSave;
    }

    getDateOfValueAsText(date: Date): string {
        let day = date.getDate().toString();
        if (day.length === 1) {
            day = '0' + day;
        }
        let month = (date.getMonth() + 1).toString();
        if (month.length === 1) {
            month = '0' + month;
        }
        const text = date.getFullYear().toString() + '-' + month + '-' + day + 'T00:00:00';
        return text;
    }

    public onCancel(): void {
        super.confirmDialog();
    }
}

/**
 * Class to represent confirm dialog model.
 *
 * It has been kept here to keep it as part of shared component.
 */
export class HistoryDialogModel {
    constructor(public value: string, public type: HistoryElementType) {
    }
  }
