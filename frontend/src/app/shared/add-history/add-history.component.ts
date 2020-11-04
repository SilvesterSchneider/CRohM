import { OnInit, Component } from '@angular/core';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { HistoryElementCreateDto, HistoryElementType } from 'src/app/shared/api-generated/api-generated';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { BaseDialogInput } from '../form/base-dialog-form/base-dialog.component';
import { MatMonthView } from '@angular/material/datepicker';

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
            date: [new FormControl(new Date(this.getDate())), Validators.required],
            information: ['', Validators.required],
            comment: ['', Validators.required]
        });
        this.oppoSuitsForm.get('date').setValue(this.getDate());
    }

    getDate(): string {
        const date = new Date(Date.now());
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
        const date = new Date(this.oppoSuitsForm.get('date').value);
        historyToSave = {
            date: this.getDateOfValue(date),
            name: this.oppoSuitsForm.get('information').value,
            type: typeToSave,
            comment: this.oppoSuitsForm.get('comment').value,
        };
        return historyToSave;
    }

    getDateOfValue(date: Date): string {
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
