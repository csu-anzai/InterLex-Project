import {NgModule, Optional, SkipSelf} from '@angular/core';
import {CommonModule} from '@angular/common';
import {HttpService} from './services/http.service';
import {MenubarModule} from 'primeng/menubar';
import {NavigationComponent} from './navigation/navigation.component';
import {InputTextareaModule, InputTextModule, SplitButtonModule, ToolbarModule} from 'primeng/primeng';
import {CalendarModule} from 'primeng/calendar';
import {AlertComponent} from './alert/alert.component';
import {AlertService} from './services/alert.service';
import {AuthGuard} from './guards/auth.guard';
import {HTTP_INTERCEPTORS} from '@angular/common/http';
import {ErrorInterceptor} from './interceptors/error.interceptor';
import {EditorModule} from 'primeng/editor';
import {TokenInterceptor} from './interceptors/token.interceptor';
import {ToastModule} from 'primeng/toast';

@NgModule({
  imports: [
    InputTextModule,
    InputTextareaModule,
    CalendarModule,
    EditorModule,
    CommonModule,
    MenubarModule,
    ToolbarModule,
    SplitButtonModule,
    ToastModule
  ],
  declarations: [NavigationComponent, AlertComponent],
  providers: [HttpService, AlertService, AuthGuard, {
    provide: HTTP_INTERCEPTORS,
    useClass: ErrorInterceptor,
    multi: true
  },
    {provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true}],
  exports: [NavigationComponent, AlertComponent]
})
export class CoreModule {

  constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
    if (parentModule) {
      throw new Error('Core module is already loaded. Import it in the AppModule only!');
    }
  }
}
