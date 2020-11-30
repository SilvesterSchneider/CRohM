import { MissingTranslationHandler, MissingTranslationHandlerParams } from '@ngx-translate/core';

export class MissingTranslationLogger extends MissingTranslationHandler {
    handle(params: MissingTranslationHandlerParams): any {
        console.error(`Missing translation for ${params.key}`);
        return params.key;
    }
}

