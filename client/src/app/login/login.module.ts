import { CommonModule } from '@angular/common';
import { ModuleWithProviders, NgModule, Type } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { UiModule } from '../ui/ui.module';
import { LoginComponent } from './components/login/login.component';
import { LoginRoutingModule } from './login-routing.module';
import { LoginModuleConfiguration } from './models/login-module-configuration';
import { TokenStore } from './models/token-store';
import { AuthenticationService } from './services/authentication.service';
import { IsAuthenticatedGuard } from './services/is-authenticated.guard';

@NgModule({
  declarations: [LoginComponent],
  imports: [UiModule, LoginRoutingModule, ReactiveFormsModule, CommonModule],
})
export class LoginModule {
  static forRoot(moduleConfiguration: Type<LoginModuleConfiguration>): ModuleWithProviders {
    return {
      ngModule: LoginModule,
      providers: [
        AuthenticationService,
        TokenStore,
        IsAuthenticatedGuard,
        {
          provide: LoginModuleConfiguration,
          useClass: moduleConfiguration,
        },
      ],
    };
  }
}
