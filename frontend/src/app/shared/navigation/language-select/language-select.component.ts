import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-language-select',
  templateUrl: './language-select.component.html',
  styleUrls: ['./language-select.component.scss']
})
export class LanguageSelectComponent {

  constructor(private translate: TranslateService) { }

  public setLanguage(lang: string) {
    this.translate.use(lang);
  }

}
