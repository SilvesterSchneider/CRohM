import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Injectable({
  providedIn: 'root'
})
export class TranslationService {
  public static LANGUAGES = [
    {
      label: 'Deutsch (German)',
      short: 'de',
      locale: 'de-DE'
    },
    {
      label: 'English (English)',
      short: 'en',
      locale: 'en_US'
    }];
  private readonly LS_KEY = 'language';
  private readonly DEFAULT_LANGUAGE = TranslationService.LANGUAGES[0];

  constructor(private translate: TranslateService) { }

  public setDefaultLanguage() {
    this.translate.setDefaultLang(this.getLanguage());
  }

  public getLanguage(): string {
    return localStorage.getItem(this.LS_KEY) ?? this.DEFAULT_LANGUAGE.short;
  }

  public setLanguage(language: string) {
    localStorage.setItem(this.LS_KEY, language);
    this.translate.use(language);
  }

  public getLocale() {
    const test = TranslationService.LANGUAGES.find((lang) => lang.short === this.getLanguage()).locale ?? this.DEFAULT_LANGUAGE.locale;
    console.log(test);
    return test;
  }
}
