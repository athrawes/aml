CXX = g++
CXX_FLAGS = -Icompiler/include -Wall -Wextra -std=c++23

CXX_SOURCES = $(wildcard compiler/src/*.cpp) $(wildcard compiler/src/**/*.cpp)
CXX_OBJECTS = $(patsubst compiler/src/%.cpp,build/objs/%.o,$(CXX_SOURCES))

CXX_DISTRIBUTE_FLAGS = -O2
CXX_DEBUG_FLAGS = -g

COMPILER_NAME = aml

TRASH_OBJECTS = $(wildcard build/objs/*.o) $(wildcard $(COMPILER_NAME))

make_dirs:
	mkdir -p build/objs

build/objs/%.o: compiler/src/%.cpp
	$(CXX) $(CXX_FLAGS) -c $< -o $@

all: CXX_FLAGS += $(CXX_DISTRIBUTE_FLAGS)
all: make_dirs $(CXX_OBJECTS)
	$(CXX) $(CXX_FLAGS) $(CXX_OBJECTS) -o build/$(COMPILER_NAME)

debug: CXX_FLAGS += $(CXX_DEBUG_FLAGS)
debug: make_dirs $(CXX_OBJECTS)
	$(CXX) $(CXX_FLAGS) $(CXX_OBJECTS) -o build/$(COMPILER_NAME)

clean:
	rm $(TRASH_OBJECTS)

.PHONY: clean