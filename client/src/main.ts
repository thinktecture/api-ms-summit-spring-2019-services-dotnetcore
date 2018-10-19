import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { environment } from './environments/environment';
import { BaseModule } from './app/base/base.module';

if (environment.production) {
  enableProdMode();
}

platformBrowserDynamic().bootstrapModule(BaseModule)
  .catch(err => console.error(err));
