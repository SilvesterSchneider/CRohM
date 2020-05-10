import { Component, OnInit, forwardRef, ViewChild } from '@angular/core';
import { Validators, FormGroup, FormArray, FormBuilder,
  ControlValueAccessor, Validator, NG_VALUE_ACCESSOR, NG_VALIDATORS, AbstractControl } from '@angular/forms';
import { ContactPossibilitiesEntryCreateDto, ContactPossibilitiesEntryDto } from '../api-generated/api-generated';

@Component({
    selector: 'app-contact-possibilities',
    templateUrl: './contact-possibilities.component.html',
    styleUrls: ['./contact-possibilities.component.scss'],
    providers: [
        {
          provide: NG_VALUE_ACCESSOR,
          useExisting: forwardRef(() => ContactPossibilitiesComponent),
          multi: true
        },
        {
          provide: NG_VALIDATORS,
          useExisting: forwardRef(() => ContactPossibilitiesComponent),
          multi: true
        }
      ]
})

export class ContactPossibilitiesComponent implements OnInit, ControlValueAccessor, Validator {
  public myForm: FormGroup = this.fb.group(
    {
      contactPossibilitiesEntries: this.fb.array([])
    });
  oldEntries: ContactPossibilitiesEntryDto[] = new Array();
  entriesToBeRemoved: number[] = new Array();

    // we will use form builder to simplify our syntax
    constructor(private fb: FormBuilder) { }

    validate(control: import('@angular/forms').AbstractControl): import('@angular/forms').ValidationErrors {
        return this.myForm.valid ? null : this.myForm.errors;
    }

    registerOnValidatorChange?(fn: () => void): void {
    }

    writeValue(val: any): void {
    if (val) {
      this.myForm.setValue(val, { emitEvent: false });
    }
  }
  registerOnChange(fn: any): void {
    this.myForm.valueChanges.subscribe(fn);
  }
  registerOnTouched(fn: any): void {
    this.myForm = fn;

  }
  setDisabledState?(isDisabled: boolean): void {
    isDisabled ? this.myForm.disable() : this.myForm.enable();
  }

    ngOnInit() {
    }

    createEntry() {
            // initialize our address
            return this.fb.group({
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

    save(model: any) {
        // call API to save customer
        console.log(model);
    }

    getFormGroup(): FormGroup {
      return this.myForm;
    }

    getContactPossibilitiesEntriesAsCreateDto(): ContactPossibilitiesEntryCreateDto[] {
      const entries: ContactPossibilitiesEntryCreateDto[] = new Array();
      this.getControls().forEach(element => {
        if (element.get('contactEntryName').valid && element.get('contactEntryValue').valid) {
          entries.push(
            {
              contactEntryName: element.value.contactEntryName,
              contactEntryValue: element.value.contactEntryValue
            }
          );
        }
      });
      return entries;
    }

    getContactPossibilitiesEntriesAsDto(): ContactPossibilitiesEntryDto[] {
      const entries: ContactPossibilitiesEntryDto[] = new Array();
      let index = 0;
      this.getControls().forEach(element => {
        let idOfElement = 0;
        if (this.oldEntries.length > index) {
          idOfElement = this.oldEntries[index].id;
        }
        if (element.get('contactEntryName').valid && element.get('contactEntryValue').valid) {
          entries.push(
            {
              id: idOfElement,
              contactEntryName: element.value.contactEntryName,
              contactEntryValue: element.value.contactEntryValue
            }
          );
        }
        index++;
      });
      return entries;
    }

    patchExistingValuesToForm(entries: ContactPossibilitiesEntryDto[]) {
      this.oldEntries = entries;
      this.myForm.get(FORMGROUPNAME).reset();
      if (entries.length > 0) {
        let index = 0;
        entries.forEach(x => {
          this.addEntry();
          this.getControls()[index].get('contactEntryName').patchValue(x.contactEntryName);
          this.getControls()[index].get('contactEntryValue').patchValue(x.contactEntryValue);
          index++;
        });
        this.removeEntry(index);
      }
    }

    getEntriesToBeRemoved(): number[] {
      return this.entriesToBeRemoved;
    }

    public isFormValid(): boolean {
      return this.myForm.valid;
    }
}

export const FORMGROUPNAME = 'contactPossibilitiesEntries';
