workspace "NewSphynx"
	architecture "x86_64"
	startproject "Sandbox"
	configurations  
	{
		"Debug", "Release"
	}

	startproject "Sandbox"
	
outputdir = "%{cfg.buildcfg}-%{cfg.system}-%{cfg.architecture}"

project "Sphynx"
	location "Sphynx"
	kind "StaticLib"
	language "C++"
	cppdialect "C++17"
	staticruntime "off"
	rtti "on"
	characterset "unicode"
	pchheader "pch.h"
	pchsource "Sphynx\\src\\pch.cpp"

	targetdir ("%{wks.location}\\build\\bin\\" .. outputdir .. "\\%{prj.name}")
	objdir ("%{wks.location}\\build\\int\\" .. outputdir .. "\\%{prj.name}")

	files
	{
		"Sphynx\\src\\**.h",
		"Sphynx\\src\\**.cpp",
	}
	includedirs
	{
		"%{prj.name}\\dep\\spdlog\\include",
		"%{prj.name}\\src\\Sphynx",
		"%{prj.name}\\dep\\glfw\\include",
		"%{prj.name}\\dep\\glad\\include",
		"%{prj.name}\\dep\\imgui",
		"%{prj.name}\\dep\\glm",
		"%{prj.name}\\dep\\stb",
		"%{prj.name}\\dep\\mono\\include",
	}
	links
	{
		"glfw",
		"glad",
		"imgui",
		"opengl32.lib",
		"%{wks.location}\\%{prj.name}\\dep\\mono\\lib\\libmono-static-sgen.lib",
	}
	defines{
		-- "SPDLL"
	}
	filter "files:**.as"
		buildaction "Copy"

	-- filter "files:**.c":
	-- 	compileas "C"
	filter "configurations:Debug"
		defines "DEBUG"
		runtime "Debug"
		symbols "on"

	filter "configurations:Release"
		defines "RELEASE"
		runtime "Release"
		optimize "off"

	filter "system:windows"
		systemversion "latest"
		links{
			"Ws2_32.lib",
			"Winmm.lib",
			"Version.lib",
			"Bcrypt.lib"
		}
	filter{}

project "Sandbox"
	location "Sandbox"
	kind "ConsoleApp"
	language "C++"
	cppdialect "C++17"
	staticruntime "off"

	targetdir ("%{wks.location}\\build\\bin\\" .. outputdir .. "\\%{prj.name}")
	objdir ("%{wks.location}\\build\\int\\" .. outputdir .. "\\%{prj.name}")

	files
	{
		"Sandbox\\src\\**.h",
		"Sandbox\\src\\**.cpp",
		"Sandbox\\src\\**.lua"
	}

	includedirs
	{
		"%{wks.location}\\Sphynx\\dep\\spdlog\\include",
		"%{wks.location}\\Sphynx\\dep\\glm",
		"%{wks.location}\\Sphynx\\src\\Sphynx",
		"%{wks.location}\\Sphynx\\src",
	}

	links
	{
		"Sphynx",
		"ScriptAssembly",
		"GameAssembly",
		"%{wks.location}\\Sphynx\\dep\\mono\\lib\\libmono-static-sgen.lib",
	}

	copylocal{
		"ScriptAssembly",
		"GameAssembly"
	}

	filter "configurations:Debug"
		defines "DEBUG"
		runtime "Debug"
		symbols "on"

	filter "configurations:Release"
		defines "RELEASE"
		runtime "Release"
		optimize "on"
	
	filter "system:windows"
		systemversion "latest"
		links{
			"Ws2_32.lib",
			"Winmm.lib",
			"Version.lib",
			"Bcrypt.lib"
		}
   		postbuildcommands { "xcopy \"%{wks.location}%{prj.name}\\data\" \"%{wks.location}\\build\\bin\\" .. outputdir .. "\\%{prj.name}\\data\" /e/i/s/y/h/k/c" }
	filter { "not system:windows" }
   		postbuildcommands { "cp -f -r d%{wks.location}%{prj.name}\\data %{wks.location}\\build\\bin\\" .. outputdir .. "\\%{prj.name}\\data" }

include "ScriptAssembly/ScriptAssembly.lua"
include "GameAssembly/GameAssembly.lua"
include "Sphynx/dep/glfw"
include "Sphynx/dep/glad/glad"
include "Sphynx/dep/imgui"