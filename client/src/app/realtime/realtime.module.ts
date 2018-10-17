import { ModuleWithProviders, NgModule, Type } from '@angular/core';
import { LoginModule } from '../login/login.module';
import { RealtimeModuleConfiguration } from './models/realtime-module-configuration';
import { RealtimeService } from './services/realtime.service';

@NgModule({
  imports: [LoginModule],
})
export class RealtimeModule {
  static forRoot(moduleConfiguration: Type<RealtimeModuleConfiguration>): ModuleWithProviders {
    return {
      ngModule: RealtimeModule,
      providers: [
        RealtimeService,
        {
          provide: RealtimeModuleConfiguration,
          useClass: moduleConfiguration,
        },
      ],
    };
  }
}
