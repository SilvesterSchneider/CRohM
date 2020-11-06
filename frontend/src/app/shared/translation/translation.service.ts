import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Injectable({
  providedIn: 'root'
})
export class TranslationService {
  public static LANGUAGES = [
    {
      label: 'Deutsch (German)',
      short: 'de'
    },
    {
      label: 'English (English)',
      short: 'en'
    }];
  private readonly LS_KEY = 'language';
  private readonly DEFAULT_LANGUAGE = TranslationService.LANGUAGES[0].short;

  constructor(private translate: TranslateService) { }

  public setDefaultLanguage() {
    this.translate.setDefaultLang(this.getLanguage());
  }

  public getLanguage(): string {
    return localStorage.getItem(this.LS_KEY) ?? this.DEFAULT_LANGUAGE;
  }

  public setLanguage(language: string) {
    localStorage.setItem(this.LS_KEY, language);
    this.translate.use(language);
  }
}
