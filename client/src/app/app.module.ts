import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DATE_PIPE_DEFAULT_OPTIONS } from '@angular/common';
import { MAT_DATE_LOCALE } from '@angular/material/core';
import de from 'date-fns/locale/de';
import { MaterialModule } from './common/material.module';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
	declarations: [AppComponent],
	imports: [
		BrowserModule,
		AppRoutingModule,
		BrowserAnimationsModule,
		MaterialModule,
		HttpClientModule,
		FormsModule,
		ReactiveFormsModule,
	],
	providers: [
		{ provide: DATE_PIPE_DEFAULT_OPTIONS, useValue: { timezone: 'Europe/Berlin' } },
		{ provide: MAT_DATE_LOCALE, useValue: de },
	],
	bootstrap: [AppComponent],
})
export class AppModule {}
