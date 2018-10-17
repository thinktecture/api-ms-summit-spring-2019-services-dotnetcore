import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { LoginConfiguration, RealtimeConfiguration, TodoConfiguration } from '../../environments/environment';
import { LoginModule } from '../login/login.module';
import { TokenInterceptor } from '../login/models/token.interceptor';
import { RealtimeModule } from '../realtime/realtime.module';
import { TodoModule } from '../todo/todo.module';
import { UiModule } from '../ui/ui.module';
import { BaseRoutingModule } from './base-routing.module';
import { AppBarComponent } from './components/app-bar/app-bar.component';
import { HomeComponent } from './components/home/home.component';
import { NavigationComponent } from './components/navigation/navigation.component';
import { RootComponent } from './components/root/root.component';
import { BlurService } from './services/blur.service';
import { RetryInterceptor } from './services/retry.interceptor';

@NgModule({
  declarations: [
    RootComponent,
    NavigationComponent,
    HomeComponent,
    AppBarComponent,
  ],
  imports: [
    BrowserModule,
    BaseRoutingModule,
    LoginModule.forRoot(LoginConfiguration),
    HttpClientModule,
    UiModule,
    TodoModule.forRoot(TodoConfiguration),
    RealtimeModule.forRoot(RealtimeConfiguration),
  ],
  providers: [
    BlurService,
    {
      provide: HTTP_INTERCEPTORS,
      multi: true,
      useClass: RetryInterceptor,
    },
    {
      provide: HTTP_INTERCEPTORS,
      multi: true,
      useClass: TokenInterceptor,
    },
  ],
  bootstrap: [RootComponent],
})
export class BaseModule {
}
