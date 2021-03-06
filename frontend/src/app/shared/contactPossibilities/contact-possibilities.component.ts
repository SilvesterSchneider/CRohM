import { Component, OnInit, forwardRef } from '@angular/core';
import { Validators, FormGroup, FormArray, FormBuilder,
     AbstractControl,
     ValidatorFn} from '@angular/forms';
import { ContactPossibilitiesEntryDto } from '../api-generated/api-generated';

@Component({
    selector: 'app-contact-possibilities',
    templateUrl: './contact-possibilities.component.html',
    styleUrls: ['./contact-possibilities.component.scss']
})

export class ContactPossibilitiesComponent implements OnInit {
  public myForm: FormGroup = this.fb.group(
    {
      contactPossibilitiesEntries: this.fb.array([])
    });
  oldEntries: ContactPossibilitiesEntryDto[] = new Array();
  entriesToBeRemoved: number[] = new Array();

    // we will use form builder to simplify our syntax
    constructor(private fb: FormBuilder) { }

    ngOnInit() {
    }

    createEntry() {
            // initialize our address
            return this.fb.group({
                id: ['0'],
                contactEntryName: ['', Validators.required],
                contactEntryValue: ['', Validators.required]
            });
        }

    addEntry() {
        // add address to the list
        (this.myForm.get(FORMGROUPNAME) as FormArray).push(this.createEntry());
    }

    removeEntry(i: number) {
        // remove address from the list
        (this.myForm.get(FORMGROUPNAME) as FormArray).removeAt(i);
        if (this.oldEntries.length > i) {
          this.entriesToBeRemoved.push(this.oldEntries[i].id);
          this.oldEntries.splice(i, 1);
        }
    }

    getControls(): AbstractControl[] {
        return (this.myForm.get(FORMGROUPNAME) as FormArray).controls;
      }

    getForm(index: number): FormGroup {
      return this.getControls()[index] as FormGroup;
    }

    getFormGroup(): FormGroup {
      return this.myForm.get(FORMGROUPNAME) as FormGroup;
    }

    patchExistingValuesToForm(entries: ContactPossibilitiesEntryDto[]) {
      this.oldEntries = entries;
      this.myForm.get(FORMGROUPNAME).reset();
      if (entries.length > 0) {
        let index = 0;
        entries.forEach(x => {
          this.addEntry();
          this.getControls()[index].get('id').patchValue(x.id);
          this.getControls()[index].get('contactEntryName').patchValue(x.contactEntryName);
          this.getControls()[index].get('contactEntryValue').patchValue(x.contactEntryValue);
          index++;
        });
        this.removeEntry(index);
      }
    }
}

export const FORMGROUPNAME = 'contactPossibilitiesEntries';
